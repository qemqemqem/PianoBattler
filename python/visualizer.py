import numpy as np
import pygame

# Initialize pygame
pygame.init()

# Create a window
win_width, win_height = 800, 600
win = pygame.display.set_mode((win_width, win_height))
pygame.display.set_caption("Frequency Spectrum")


min_freq = 85
max_freq = 4000


def visualize_spectrum(freqs, fft_data):
    # Draw frequency bars
    win.fill((0, 0, 0))

    indices = np.where((freqs >= min_freq) & (freqs <= max_freq))[0]
    freqs_subset = freqs[indices]
    fft_data_subset = fft_data[indices]

    threshold = np.max(fft_data_subset) * 0.0

    min_log_freq = np.log10(min_freq)
    max_log_freq = np.log10(max_freq)
    log_range = max_log_freq - min_log_freq

    for i in range(len(freqs_subset)):
        freq = freqs_subset[i]

        # if fft_data_subset[i] < threshold:
        #     continue

        log_freq = np.log10(freq)
        x = int(((log_freq - min_log_freq) / log_range) * win_width)

        magnitude_dB = 10 * np.log10(fft_data_subset[i] + 1e-10)
        scaled_magnitude = int(magnitude_dB * win_height / 100)

        y = win_height - scaled_magnitude

        pygame.draw.rect(win, (0, 255, 0), (x, y, 2, scaled_magnitude))  # Bar width is set to 2 for visibility

    pygame.display.update()