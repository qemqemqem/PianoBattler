import pyaudio
import numpy as np
from scipy.interpolate import interp1d
import pygame
import sys

# Constants
FORMAT = pyaudio.paFloat32
CHANNELS = 1
RATE = 44100
CHUNK = int(RATE * 0.03)  # 30ms of audio
WIDTH, HEIGHT = 1100, 800  # Window size

# Initialize PyAudio
p = pyaudio.PyAudio()

# Open stream
stream = p.open(format=FORMAT, channels=CHANNELS, rate=RATE, input=True, frames_per_buffer=CHUNK)

# Initialize Pygame
pygame.init()
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption('Frequency Spectrum')


class FrequencyHistory:
    def __init__(self, num_frequencies, fast_smoothing_factor=0.7, slow_smoothing_factor=0.95, decay_rate=0.01):
        """
        num_frequencies: Number of frequency bins.
        fast_smoothing_factor: Smoothing factor for the fast average (closer to 1 for slower decay).
        slow_smoothing_factor: Smoothing factor for the slow average.
        decay_rate: Decay rate for the slow average.
        """
        self.fast_history = np.zeros(num_frequencies)
        self.slow_history = np.zeros(num_frequencies)
        self.fast_smoothing_factor = fast_smoothing_factor
        self.slow_smoothing_factor = slow_smoothing_factor
        self.decay_rate = decay_rate

    def update(self, current_frame):
        """
        Update the history with the current frame data.
        Applies different exponential smoothing to fast and slow histories.
        """
        self.fast_history = self.fast_smoothing_factor * self.fast_history + (
                    1 - self.fast_smoothing_factor) * current_frame
        self.slow_history = self.slow_smoothing_factor * self.slow_history * (1 - self.decay_rate) + (
                    1 - self.slow_smoothing_factor) * current_frame

    def get_smoothed_frequency(self, smoothing_width=3):
        """
        Return the frequency data after subtracting the slow smoothed rolling average from the fast one,
        and then applying frequency-domain smoothing.
        smoothing_width: Number of neighboring frequencies to include in the smoothing.
        """
        # Subtract the slow average from the fast average
        diff_average = self.fast_history - self.slow_history

        # Apply frequency-domain smoothing
        smoothed = np.convolve(diff_average, np.ones(smoothing_width) / smoothing_width, mode='same')
        return smoothed


# TODO a better way to get this number
frequency_history = FrequencyHistory(num_frequencies=512, decay_rate=0.01)



def find_fundamental_frequency(audio_data, sample_rate):
    # Perform autocorrelation
    corr = np.correlate(audio_data, audio_data, mode='full')
    corr = corr[len(corr)//2:]  # Keep the second half

    # Find the first peak
    d = np.diff(corr)
    start = np.nonzero(d > 0)[0]
    start = start[0]
    peak = np.argmax(corr[start:]) + start
    period = peak / sample_rate

    # Return fundamental frequency
    if period > 0:
        return 1.0 / period
    return 0

def convert_to_decibels(fft_magnitude):
    # Avoid log of zero by adding a small number
    magnitude_db = 20 * np.log10(fft_magnitude + 1e-6)
    return np.clip(magnitude_db, -60, None)  # Clip values below -60 dB

def a_weighting_and_bandpass_filter(frequencies):
    """ Calculate A-weighting for each frequency and apply bandpass filtering. """
    # Constants for A-weighting
    c1 = 20.6 ** 2
    c2 = 107.7 ** 2
    c3 = 737.9 ** 2
    c4 = 12200 ** 2

    # A-weighting calculation
    numerator = c4 * frequencies ** 4
    denominator = ((frequencies ** 2 + c1) * np.sqrt((frequencies ** 2 + c2) * (frequencies ** 2 + c3)) * (frequencies ** 2 + c4))
    a_weighting = numerator / denominator

    # Apply bandpass filter
    low_cutoff = 20  # 20 Hz
    high_cutoff = 20000  # 20 kHz
    a_weighting[(frequencies < low_cutoff) | (frequencies > high_cutoff)] = 0

    return a_weighting


def draw_bar(screen, x, y, width, height, color):
    pygame.draw.rect(screen, color, (x, y, width, height))

try:
    while True:
        # Handle Pygame events
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                stream.stop_stream()
                stream.close()
                p.terminate()
                pygame.quit()
                sys.exit()

        # Read audio stream
        data = stream.read(CHUNK)
        # Convert to NumPy array
        audio_data = np.frombuffer(data, dtype=np.float32)

        # Find and print fundamental frequency
        frequency = find_fundamental_frequency(audio_data, RATE)
        print(f"Fundamental Frequency: {frequency:.2f} Hz")

        # Compute FFT
        fft_data = np.fft.rfft(audio_data)
        fft_freq = np.fft.rfftfreq(CHUNK, d=1./RATE)
        fft_magnitude = np.abs(fft_data)

        # Logarithmic scale frequency bins
        min_freq = 200  # Minimum frequency (e.g., 20 Hz)
        max_freq = RATE / 2  # Nyquist frequency
        num_bins = 512  # Number of bins in the logarithmic scale
        log_freq = np.logspace(np.log10(min_freq), np.log10(max_freq), num_bins)

        # Interpolate the FFT data onto the logarithmic scale
        magnitude_interpolator = interp1d(fft_freq, fft_magnitude, bounds_error=False, fill_value=0)
        log_magnitude = magnitude_interpolator(log_freq)

        # Apply A-weighting to the FFT magnitudes
        a_weighted_fft = log_magnitude * a_weighting_and_bandpass_filter(log_freq)

        # Convert to decibels
        freqs_in_decibels = convert_to_decibels(a_weighted_fft)

        # Smoothing
        frequency_history.update(freqs_in_decibels)
        smoothed_frequency = frequency_history.get_smoothed_frequency()

        display_freqs = smoothed_frequency

        # Norrmalize 0-1
        display_freqs = display_freqs / max(np.max(display_freqs), 45)

        # Clear screen
        screen.fill((0, 0, 0))

        # Draw bars for each frequency
        bar_width = 2 # WIDTH // len(display_freqs)
        for i, freq in enumerate(display_freqs):
            x = i * bar_width
            y = HEIGHT
            # height = fft_magnitude[i] / 100 * HEIGHT # Scale for display
            height = np.interp(display_freqs[i], [0, 1], [0, HEIGHT])  # Scale dB to screen height
            draw_bar(screen, x, HEIGHT - height, bar_width + 1, height, (0, 255, 0))

        pygame.display.flip()

except KeyboardInterrupt:
    # Stop and close the stream and PyAudio
    stream.stop_stream()
    stream.close()
    p.terminate()
    pygame.quit()
    sys.exit()
