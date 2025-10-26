#!/usr/bin/env python3
"""
Region-based OCR extraction for Pokemon Go raid screenshots.

Extracts structured data from gym screenshots using Tesseract OCR
and coordinate-based region cropping from JTessBoxEditor format.
"""

import argparse
import json
import re
from pathlib import Path
from PIL import Image
import pytesseract


class BoxParser:
    """Parse JTessBoxEditor box files with coordinate conversion."""

    def __init__(self, image_height):
        """
        Initialize parser with image dimensions for coordinate conversion.

        Args:
            image_height: Height of the image in pixels
        """
        self.image_height = image_height
        self.regions = {}

    def parse_box_file(self, box_file_path):
        """
        Parse box file and convert coordinates from JTessBoxEditor format.

        JTessBoxEditor uses: symbol x_left y_bottom x_right y_top page
        Need to convert to standard image coordinates (top-left origin).

        Args:
            box_file_path: Path to the .box file

        Returns:
            Dictionary mapping region names to crop boxes (x, y, width, height)
        """
        with open(box_file_path, 'r') as f:
            for line in f:
                line = line.strip()
                if not line:
                    continue

                # Parse: label x_left y_bottom x_right y_top page
                parts = line.split()
                if len(parts) >= 5:
                    label = parts[0]
                    x_left = int(parts[1])
                    y_bottom = int(parts[2])
                    x_right = int(parts[3])
                    y_top = int(parts[4])

                    # Convert JTessBoxEditor coordinates to standard image coordinates
                    # JTessBoxEditor: bottom-left origin (y increases upward)
                    # Standard: top-left origin (y increases downward)
                    x = x_left
                    y = self.image_height - y_top  # Convert from bottom to top
                    width = x_right - x_left
                    height = y_top - y_bottom

                    self.regions[label] = {
                        'x': x,
                        'y': y,
                        'width': width,
                        'height': height,
                        'original_coords': (x_left, y_bottom, x_right, y_top)
                    }

        return self.regions


class RaidDataExtractor:
    """Extract structured raid data from Pokemon Go screenshots."""

    def __init__(self, image_path, box_file_path):
        """
        Initialize extractor with image and box file.

        Args:
            image_path: Path to the screenshot image
            box_file_path: Path to the box file with region coordinates
        """
        self.image = Image.open(image_path)
        self.image_height = self.image.height
        self.image_width = self.image.width

        # Parse box file
        parser = BoxParser(self.image_height)
        self.regions = parser.parse_box_file(box_file_path)

        # Store tier template once it's extracted
        self.tier_template = None

    def crop_region(self, region_name):
        """
        Crop a region from the screenshot.

        Args:
            region_name: Name of the region from box file

        Returns:
            PIL Image object of the cropped region
        """
        if region_name not in self.regions:
            raise ValueError(f"Region '{region_name}' not found in box file")

        region = self.regions[region_name]
        box = (
            region['x'],
            region['y'],
            region['x'] + region['width'],
            region['y'] + region['height']
        )
        return self.image.crop(box)

    def extract_text_region(self, region_name):
        """
        Extract text from a region using Tesseract OCR.

        Args:
            region_name: Name of the region to extract text from

        Returns:
            Extracted text as string
        """
        cropped = self.crop_region(region_name)

        # Preprocess image for better OCR
        import cv2
        import numpy as np

        # Convert PIL to numpy array
        img = np.array(cropped.convert('RGB'))

        # Convert to grayscale
        gray = cv2.cvtColor(img, cv2.COLOR_RGB2GRAY)

        # Apply thresholding to get clear black and white
        _, thresh = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)

        # Resize if image is too small (Tesseract works better on larger images)
        if thresh.shape[0] < 50:
            scale = 50 / thresh.shape[0]
            width = int(thresh.shape[1] * scale)
            height = int(thresh.shape[0] * scale)
            thresh = cv2.resize(thresh, (width, height), interpolation=cv2.INTER_CUBIC)

        # Run OCR with optimized config
        text = pytesseract.image_to_string(thresh, config='--psm 7 -c tessedit_char_whitelist=0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz :')
        return text.strip()

    def detect_tier(self):
        """
        Detect the tier (1-5) by analyzing skull icons in tier regions.

        Returns:
            Tier number (1-5)
        """
        try:
            import cv2
            import numpy as np
        except ImportError:
            # Fallback: simple pixel-based detection
            print("OpenCV not available, using fallback tier detection")
            return self._detect_tier_fallback()

        # Extract tier template from first region if not already extracted
        if self.tier_template is None:
            tier_one_crop = self.crop_region('tier_label_one')
            self.tier_template = np.array(tier_one_crop.convert('RGB'))

            # Create a simple binary mask of non-transparent/non-background pixels
            # Assume skulls are darker than background
            gray_template = cv2.cvtColor(self.tier_template, cv2.COLOR_RGB2GRAY)
            _, self.tier_template = cv2.threshold(gray_template, 200, 255, cv2.THRESH_BINARY_INV)

        # Check all 5 tier regions for skull presence
        tier = 0
        for i in range(1, 6):
            region_name = f'tier_label_{self._number_to_word(i)}'

            if region_name not in self.regions:
                continue

            tier_crop = self.crop_region(region_name)
            tier_img = np.array(tier_crop.convert('RGB'))
            gray = cv2.cvtColor(tier_img, cv2.COLOR_RGB2GRAY)
            _, binary = cv2.threshold(gray, 200, 255, cv2.THRESH_BINARY_INV)

            # Count non-white pixels (presence of skull)
            non_white_pixels = np.sum(binary > 0)

            # Threshold: if significant pixels found, tier is present
            if non_white_pixels > 100:
                tier = i

        return tier if tier > 0 else 1

    def _detect_tier_fallback(self):
        """
        Fallback tier detection using simple pixel analysis.

        Returns:
            Tier number (1-5)
        """
        tier = 0
        for i in range(1, 6):
            region_name = f'tier_label_{self._number_to_word(i)}'

            if region_name not in self.regions:
                continue

            tier_crop = self.crop_region(region_name)

            # Convert to RGB and analyze pixels
            pixels = list(tier_crop.convert('RGB').getdata())

            # Count non-white pixels (skulls are darker)
            non_white = sum(1 for r, g, b in pixels if not (r > 240 and g > 240 and b > 240))

            # If significant non-white pixels, assume skull is present
            if non_white > len(pixels) * 0.1:
                tier = i

        return tier if tier > 0 else 1

    def _number_to_word(self, num):
        """Convert number to word for region names."""
        words = {1: 'one', 2: 'two', 3: 'three', 4: 'four', 5: 'five'}
        return words.get(num, str(num))

    def extract_pokemon_name(self):
        """Extract Pokemon name from pokemon_name_label region."""
        text = self.extract_text_region('pokemon_name_label')
        # Clean up OCR result
        text = self._clean_text(text)
        return text.lower().replace(' ', '')

    def extract_gym_name(self):
        """Extract gym name from gym_name_label region."""
        text = self.extract_text_region('gym_name_label')
        text = self._clean_text(text)
        return text

    def extract_combat_power(self):
        """Extract combat power from combat_power_label region."""
        text = self.extract_text_region('combat_power_label')
        # Extract all consecutive digits
        match = re.search(r'\d{4,}', text)
        if match:
            return int(match.group())
        # Fallback: try to extract any numbers
        numbers = re.sub(r'[^\d]', '', text)
        return int(numbers) if numbers else 0

    def extract_time_remaining(self):
        """Extract time remaining from time_remaining_label region."""
        text = self.extract_text_region('time_remaining_label')
        # Format: HH:MM:SS or MM:SS
        time_pattern = r'(\d{2}):(\d{2})(?::(\d{2}))?'
        match = re.search(time_pattern, text)
        if match:
            hours, minutes, seconds = match.groups()
            if seconds is None:
                seconds = '00'
            return f"{hours}:{minutes}:{seconds}"
        return text.strip()

    def extract_group_type(self):
        """Extract group type from group_type_label region."""
        text = self.extract_text_region('group_type_label')
        text = self._clean_text(text)
        return text.lower()

    def _clean_text(self, text):
        """Clean up OCR text output."""
        # Remove extra whitespace and special characters
        text = ' '.join(text.split())
        # Remove common OCR errors
        text = text.replace('|', 'I')
        # Only replace 0 with O if it's clearly in a word context
        text = text.replace('®', '').replace('™', '').replace('©', '')
        return text

    def extract_all(self):
        """
        Extract all raid data from the screenshot.

        Returns:
            Dictionary with all extracted data
        """
        print("Extracting raid data from screenshot...")

        try:
            pokemon_name = self.extract_pokemon_name()
            print(f"  Pokemon: {pokemon_name}")
        except Exception as e:
            print(f"  Error extracting pokemon name: {e}")
            pokemon_name = "unknown"

        try:
            tier = self.detect_tier()
            print(f"  Tier: {tier}")
        except Exception as e:
            print(f"  Error detecting tier: {e}")
            tier = 1

        try:
            gym_name = self.extract_gym_name()
            print(f"  Gym: {gym_name}")
        except Exception as e:
            print(f"  Error extracting gym name: {e}")
            gym_name = "Unknown Gym"

        try:
            combat_power = self.extract_combat_power()
            print(f"  CP: {combat_power}")
        except Exception as e:
            print(f"  Error extracting combat power: {e}")
            combat_power = 0

        try:
            time_remaining = self.extract_time_remaining()
            print(f"  Time: {time_remaining}")
        except Exception as e:
            print(f"  Error extracting time remaining: {e}")
            time_remaining = "00:00:00"

        try:
            group_type = self.extract_group_type()
            print(f"  Group type: {group_type}")
        except Exception as e:
            print(f"  Error extracting group type: {e}")
            group_type = "unknown"

        return {
            "pokemon_name": pokemon_name,
            "tier": tier,
            "gym_name": gym_name,
            "combat_power": combat_power,
            "time_remaining": time_remaining,
            "group_type": group_type
        }


def main():
    """Main entry point for the extraction script."""
    parser = argparse.ArgumentParser(
        description='Extract structured raid data from Pokemon Go screenshots'
    )
    parser.add_argument('image_path', help='Path to the screenshot image')
    parser.add_argument('box_file', help='Path to the box file with coordinates')
    parser.add_argument('-o', '--output', default='raid_data.json',
                       help='Output JSON file path (default: raid_data.json)')
    parser.add_argument('--tesseract-cmd', help='Path to tesseract binary')
    parser.add_argument('--debug', action='store_true',
                       help='Save debug crops and show region info')

    args = parser.parse_args()

    # Set Tesseract path if provided
    if args.tesseract_cmd:
        pytesseract.pytesseract.tesseract_cmd = args.tesseract_cmd

    # Check if files exist
    if not Path(args.image_path).exists():
        print(f"Error: Image file not found: {args.image_path}")
        return 1

    if not Path(args.box_file).exists():
        print(f"Error: Box file not found: {args.box_file}")
        return 1

    # Extract data
    extractor = RaidDataExtractor(args.image_path, args.box_file)

    # Debug mode: save cropped regions
    if args.debug:
        print("\nDebug mode: Saving cropped regions...")
        debug_dir = Path('debug_crops')
        debug_dir.mkdir(exist_ok=True)

        for region_name in extractor.regions.keys():
            cropped = extractor.crop_region(region_name)
            debug_path = debug_dir / f"{region_name}_crop.png"
            cropped.save(debug_path)
            print(f"  Saved: {debug_path}")

    raid_data = extractor.extract_all()

    # Save to JSON
    output_path = Path(args.output)
    with open(output_path, 'w') as f:
        json.dump(raid_data, f, indent=2)

    print(f"\nExtraction complete! Results saved to: {output_path}")
    return 0


if __name__ == '__main__':
    exit(main())

