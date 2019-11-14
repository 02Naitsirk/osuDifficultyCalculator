# What is this?
An experimental project (with bad code) not meant to be taken seriously. It calculates the SR and pp values of osu! beatmaps without using strain, which measures how hard it is to keep up with a pattern. Instead, each object receives a difficulty value based on its distance and time to the previous note. The difficulty values are then added up in a weighted sum, and then converted to SR and pp.

# What can this not do?
All this does is calculate SR and pp. There's no code to calculate pp with a certain accuracy, combo, misses, etc. However, I would recommend the following formula:

`pp = (SS pp) * ARCCOS(1 - 2 * ACCURACY) * SQRT(COMBO / MAX COMBO) * 0.95^(MISSES) / (PI * (2 - ACCURACY)).`

Also, there's no EZ, you must manually adjust the .osu file. Simply divide the CS, AR, and OD by 2, and don't round.

# Results
Because this system is strainless, you can expect difficulty spikes to be nerfed, because strain becomes high on difficulty spikes, and  consistent maps to be buffed.
Here are some interesting results:

Map|SR|SS pp
---|---|---
Camellia vs Akira Complex - Reality Distortion (rrtyui) [rrtyui x Sing's Transference] | 7.67* | 584.54pp
Camellia - Exit This Earth's Atomosphere (rrtyui) [Evolution] | 7.94* | 636.46pp
Culprate & Au5 - Impulse (handsome) [Master] | 7.75* | 612.47pp
gmtn. (witch's slave) - furioso melodia (Alumetorz) [Wrath] | 7.62* | 661.96pp
Grabbitz - Way Too Deep (UndeadCapulet) [Settling Down] | 8.36* | 829.44pp
Halozy - Deconstruction Star (Hollow Wings) [Beat Heaven] | 8.47* | 800.18pp
M2U - The back of Beyond (yaspo) [Spring] | 7.48* | 458.74pp
sakuzyo - AXION (Flower) [AXION_REBORN] | 7.63* | 496.78pp
Sota Fujimori - polygon (Kaifin) [Bonzi's Ultra] | 8.17* | 616.39pp 
Squarepusher - Dark Steering (dsco) [Nuclear] | 7.81* | 692.63pp

The system nerfs some short or farmy maps:

Map|SR|SS pp|DT SR|DT pp
---|---|---|---|---
Will Stetson - Harumachi Clover (Swing Arrangement) (Will Stetson) [Fiery's Extreme] | 5.16* | 167.75pp | 7.19* | 417.80pp
Nanahira - Chikatto Chika Chika (Agatsu) [Sotarks' 1+2 IQ] | 5.98* | 270.76pp | 8.35* | 676.73pp
Will Stetson - Snow Halation (feat. BeasttrollMC) (Sotarks) [Nevo's Extra] | 5.44* | 207.84pp | 7.59* | 520.43pp
Vickeblanka - Black Rover (TV Size) (Sotarks) [Extra] | 6.12* | 273.83pp | 8.59* | 711.32pp
TWICE - CHEER UP (Sotarks) [SHY SHY SHY!] | 5.69* | 232.64pp | 7.95* | 593.01pp
Parry Gripp - Guinea Pig Bridge (Sotarks) [Guinea Pig Technology] | 5.55* | 206.43pp | 7.75* | 521.87pp
S3RL - Bass Slut (Original Mix) (Fatfan Kolek) [Taeyang's Trashy Extreme] | 5.90* | 260.76pp | 8.23* | 660.68pp

# Download:
* [Download .exe file.](https://naitsirk.s-ul.eu/6K6bQNyZ.exe)
