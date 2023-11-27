import pyaudio
import numpy as np

# Constants
FORMAT = pyaudio.paFloat32
CHANNELS = 1
RATE = 44100
CHUNK = int(RATE * 0.03)  # 30ms of audio

# Initialize PyAudio
p = pyaudio.PyAudio()

# Open stream
stream = p.open(format=FORMAT, channels=CHANNELS, rate=RATE, input=True, frames_per_buffer=CHUNK)

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

try:
    while True:
        # Read audio stream
        data = stream.read(CHUNK)
        # Convert to NumPy array
        audio_data = np.frombuffer(data, dtype=np.float32)

        # Find and print fundamental frequency
        frequency = find_fundamental_frequency(audio_data, RATE)
        print(f"Fundamental Frequency: {frequency:.2f} Hz")

except KeyboardInterrupt:
    # Stop and close the stream and PyAudio
    stream.stop_stream()
    stream.close()
    p.terminate()
