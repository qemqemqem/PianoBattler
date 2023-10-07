import pyaudio
import numpy as np
from scipy.fftpack import fft
from scipy.signal import butter, filtfilt

from python.visualizer import visualize_spectrum


# Band-pass filter
def butter_bandpass_filter(data, lowcut, highcut, rate, order=4):
    nyquist = 0.5 * rate
    low = lowcut / nyquist
    high = highcut / nyquist
    b, a = butter(order, [low, high], btype="band")
    y = filtfilt(b, a, data)
    return y


def record_and_analyze_continuous(window_length=1, rate=44100):
    # Initialize PyAudio
    audio = pyaudio.PyAudio()

    # Correct calculation for 100ms buffer length
    buffer_len = int(rate * 0.1)

    # Initialize an empty rolling buffer with zeros
    rolling_buffer = np.zeros(int(rate * window_length), dtype=np.int16)

    # Start recording
    stream = audio.open(format=pyaudio.paInt16, channels=1, rate=rate, input=True, frames_per_buffer=buffer_len)
    print("Recording... Press Ctrl+C to stop.")

    try:
        while True:
            # Record for 100ms
            data = stream.read(buffer_len)
            new_samples = np.frombuffer(data, dtype=np.int16)

            # Roll the buffer and add new samples
            rolling_buffer = np.roll(rolling_buffer, -len(new_samples))
            rolling_buffer[-len(new_samples):] = new_samples

            # Apply band-pass filter for basic noise filtering
            filtered_audio = butter_bandpass_filter(rolling_buffer, 85, 1000, rate)
            # Perform FFT to get frequency components
            fft_data = fft(filtered_audio)
            freqs = np.fft.fftfreq(len(fft_data), 1 / rate)

            # Visualize
            visualize_spectrum(freqs, fft_data)

            # Get the dominant frequency
            idx = np.argmax(np.abs(fft_data))
            dominant_freq = freqs[idx]

            print(f"Dominant frequency: {dominant_freq:.2f} Hz")

    except KeyboardInterrupt:
        print("Recording stopped.")

    finally:
        # Stop and close the stream
        stream.stop_stream()
        stream.close()
        audio.terminate()


if __name__ == "__main__":
    record_and_analyze_continuous()
