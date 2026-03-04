import os
import struct


def make_pixels(size):
    background = (20, 137, 125, 255)
    foreground = (255, 255, 255, 255)
    shadow = (12, 92, 85, 255)
    pixels = []

    for y in range(size):
        row = []
        for x in range(size):
            row.append(background)
        pixels.append(row)

    thickness = max(2, size // 8)
    margin = max(4, size // 5)
    gap = max(3, size // 6)

    for y in range(margin, size - margin):
        for x in range(margin + gap, margin + gap + thickness):
            pixels[y][x] = shadow
            pixels[y][size - margin - gap - thickness + (x - (margin + gap))] = shadow

    for y in range(margin + gap, margin + gap + thickness):
        for x in range(margin, size - margin):
            pixels[y][x] = foreground
            pixels[size - margin - gap - thickness + (y - (margin + gap))][x] = foreground

    for y in range(margin, size - margin):
        for x in range(margin, margin + thickness):
            pixels[y][x + gap] = foreground
            pixels[y][size - margin - thickness - gap + (x - margin)] = foreground

    return pixels


def build_bmp_data(size):
    pixels = make_pixels(size)
    xor_bytes = bytearray()

    for y in range(size - 1, -1, -1):
        for x in range(size):
            r, g, b, a = pixels[y][x]
            xor_bytes.extend([b, g, r, a])

    mask_row_size = ((size + 31) // 32) * 4
    and_mask = bytearray(mask_row_size * size)

    header = struct.pack(
        "<IIIHHIIIIII",
        40,
        size,
        size * 2,
        1,
        32,
        0,
        len(xor_bytes) + len(and_mask),
        0,
        0,
        0,
        0,
    )

    return header + xor_bytes + and_mask


def write_icon(path, size):
    image_data = build_bmp_data(size)
    icon_dir = struct.pack("<HHH", 0, 1, 1)
    entry = struct.pack(
        "<BBBBHHII",
        size if size < 256 else 0,
        size if size < 256 else 0,
        0,
        0,
        1,
        32,
        len(image_data),
        6 + 16,
    )

    with open(path, "wb") as handle:
        handle.write(icon_dir)
        handle.write(entry)
        handle.write(image_data)


if __name__ == "__main__":
    base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    output_path = os.path.join(base_dir, "src", "HashPDF.WinForms", "Assets", "HashPDF.ico")
    output_dir = os.path.dirname(output_path)
    if not os.path.isdir(output_dir):
        os.makedirs(output_dir)
    write_icon(output_path, 32)
