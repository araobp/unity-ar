#!/usr/bin/env python3

from PIL import Image
from pdf2image import convert_from_path
import argparse

Image.MAX_IMAGE_PIXELS = 189792256

parser = argparse.ArgumentParser(description='PDF to PNG converter with transparent background')
parser.add_argument('filename', type=str, help='PDF file name')
parser.add_argument('-i', '--inverse', help='Inverse black and white', action='store_true')
parser.add_argument('-t', '--threshold', help='Threshold', type=int, default=255)
parser.add_argument('-I', '--invert', help='Invert', action='store_true')
parser.add_argument('-4', '--four', help='Four images per page', action='store_true')
args = parser.parse_args()

# A4 aspect ratio: 297mm * 210mm in landscape
if (args.invert):
    W = 5792
    H = 8192
else:
    W = 8192
    H = 5792 

if args.four:
    W *= 2
    H *= 2

pages = convert_from_path(args.filename, size=(W, H))

file_without_ext = args.filename.split('.')[0]

for i, page in enumerate(pages):
    if i == 0:
        f = '{}'.format(file_without_ext)
    else:
        f = '{}-{:d}'.format(file_without_ext, i) 
    img = Image.new('RGBA', page.size) 
    data = page.getdata()
    newData = []
    thres = args.threshold
    for item in data:
        r = item[0]
        g = item[1]
        b = item[2]
        if r >= thres and g >= thres and b >= thres:
            newData.append((255,255,255,0))  # transparent
        else:
            if args.inverse:
                pixel = (255-r, 255-g, 255-b, 255)  # white
            else:
                pixel = (r, g, b, 255) # black
            newData.append(pixel)

    img.putdata(newData)
    img.save(f + '.png', 'PNG')

    if args.four:
        if args.invert:
            img0 = img.crop((0, 0, H/2, W/2))
            img1 = img.crop((H/2, 0, H, W/2))
            img2 = img.crop((0, W/2, H/2, W))
            img3 = img.crop((H/2, W/2, H, W))
        else:
            img0 = img.crop((0, 0, W/2, H/2))
            img1 = img.crop((W/2, 0, W, H/2))
            img2 = img.crop((0, H/2, W/2, H))
            img3 = img.crop((W/2, H/2, W, H))

        img0.save(f + '#0.png', 'PNG')
        img1.save(f + '#1.png', 'PNG')
        img2.save(f + '#2.png', 'PNG')
        img3.save(f + '#3.png', 'PNG')
