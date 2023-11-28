import pyaudio
import numpy as np
import pygame
import sys

# Constants
FORMAT = pyaudio.paFloat32
CHANNELS = 1
RATE = 44100
CHUNK = int(RATE * 0.03)  # 30ms of audio
WIDTH, HEIGHT = 800, 600  # Window size

# Initialize PyAudio
p = pyaudio.PyAudio()

# Open stream
stream = p.open(format=FORMAT, channels=CHANNELS, rate=RATE, input=True, frames_per_buffer=CHUNK)

# Initialize Pygame
pygame.init()
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption('Frequency Spectrum')


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

        # Clear screen
        screen.fill((0, 0, 0))

        # Draw bars for each frequency
        bar_width = WIDTH // len(fft_freq)
        for i, freq in enumerate(fft_freq):
            x = i * bar_width
            y = HEIGHT
            # height = fft_magnitude[i] / 100 * HEIGHT # Scale for display
            magnitude_db = convert_to_decibels(fft_magnitude[i])
            height = np.interp(magnitude_db, [0, 60], [0, HEIGHT])  # Scale dB to screen height
            draw_bar(screen, x, HEIGHT - height, bar_width + 1, height, (0, 255, 0))

        pygame.display.flip()

except KeyboardInterrupt:
    # Stop and close the stream and PyAudio
    stream.stop_stream()
    stream.close()
    p.terminate()
    pygame.quit()
    sys.exit()
