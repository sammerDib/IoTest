echo ------------------------------- >> FPTimestamp.txt
ver >> FPTimestamp.txt
vol >> FPTimestamp.txt
date /T >> FPTimestamp.txt
time /T >> FPTimestamp.txt
echo ------------------------------- >> FPTimestamp.txt
del SPGFogaleProbe.zip
"C:\Program Files\7-Zip\7z" a -tzip SPGFogaleProbe.zip -ir!*.cpp -ir!*.c -ir!*.h -ir!*.agh -ir!*.def -ir!*.rc -ir!*.rc2 -ir!*.ico -ir!*.dsp -ir!*.vcproj -ir!*.vcproj.* -ir!*.dsw -ir!*.opt -ir!*.txt -ir!*.bmp -ir!*.bat
pause