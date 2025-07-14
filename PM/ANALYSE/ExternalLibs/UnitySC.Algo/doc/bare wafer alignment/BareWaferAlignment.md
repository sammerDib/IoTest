# Bare wafer alignment

## Overview
When a wafer is set on a chuck, the wafer is not aligned with the chuck.
Both centers are different, in a magnitude relative to loading arm specification.
The bare wafer alignment algorithm detects position differences:
-  x/y center shift: difference between wafer and chuck centers
- rotation: difference between perfect angle with:
    - Notch wafer: notch at bottom center and actual notch position

The x/y shift is detected by fitting a circle using segmented wafer edges images.
For notch wafers, the angle difference is obtained using x/y shift information and using a image of wafer notch. More on this later.

## X/Y shift detection
To detect the X/Y shift, we use segmentation images of wafer edges. Wafer edge contours are isolated in each images, their point coordinates are projected to the common wafer referential and a circle fit is performed. That fit provides center position, wafer diameter and a RMSE indicator.

## Angle detection

### Notch wafer

To detect the angle, we use a polar project of an image of the notch. The pole of that projection is the wafer center, where ordinates indicates angles and abcissa indicates radius.
Once we have the polar projection, it is cropped so that lines and columns not containing enough image data are cropped out. That cropped image is then padded on top and bottom using first and last line values, respectively. We obtain an image which have twice the row count as the cropped polar image. This image is duplicated and flipped over abcissa axis. We then use these two cropped/padded images to compute a cross-correlation peak, which indicates the slide of the second image to reach the point of better similarity of two images. With some computation, we obtain the ordinate index of the peak, which itself indicates the angle of symetry axis relative to wafer center. We substract the perfect angle (270°) from that value and obtain the angle 'tilt' rotation value.
Note: for the computation of angle from two images, you can have a look at [angle_extraction.jpg](angle_extraction.jpg) which contains drawings used for unit-testing the angle extraction code.
