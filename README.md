# What is this?
An experimental project (with bad code) not meant to be taken seriously. It calculates the SR and pp values of osu! beatmaps without using decaying strain, which measures how hard it is to keep up with a pattern. Instead, each object receives a difficulty value based on its distance and time to the previous note. The difficulty values are then added up in a weighted sum, and then converted to SR and pp.

# What can this not do?
All this does is calculate SR and pp. There's no code to calculate pp with a certain accuracy, combo, misses, etc. However, I would recommend the following formula:

`pp = (SS pp) * ARCCOS(1 - 2 * ACCURACY) * SQRT(COMBO / MAX COMBO) * 0.95^(MISSES) / (PI * (2 - ACCURACY)).`

There's no EZ, you must manually adjust the .osu file. Simply divide the CS, AR, and OD by 2, and don't round.

Finally, this system does not have a speed buff.

# Results
Because this system is "strainless", you can expect difficulty spikes to be nerfed, because strain becomes high on difficulty spikes, and  consistent maps to be buffed.
Here are some interesting results:

Map|SR|SS pp
---|---|---
Camellia vs Akira Complex - Reality Distortion (rrtyui) [rrtyui x Sing's Transference] | 7.62* | 576pp
Camellia - Exit This Earth's Atomosphere (rrtyui) [Evolution] | 7.87* | 624pp
Culprate & Au5 - Impulse (handsome) [Master] | 7.20* | 512pp
gmtn. (witch's slave) - furioso melodia (Alumetorz) [Wrath] | 7.65* | 668pp
Grabbitz - Way Too Deep (UndeadCapulet) [Settling Down] | 7.95* | 736pp
Halozy - Deconstruction Star (Hollow Wings) [Beat Heaven] | 8.41* | 785pp
M2U - The back of Beyond (yaspo) [Spring] | 7.02* | 394pp
sakuzyo - AXION (Flower) [AXION_REBORN] | 7.53* | 480pp
Sota Fujimori - polygon (Kaifin) [Bonzi's Ultra] | 8.00* | 587pp
Squarepusher - Dark Steering (dsco) [Nuclear] | 7.82* | 693pp

The system nerfs some short or farmy maps:

Map|SR|SS pp|DT SR|DT pp
---|---|---|---|---
Will Stetson - Harumachi Clover (Swing Arrangement) (Will Stetson) [Fiery's Extreme] | 5.06* | 160pp | 7.04* | 397pp
Nanahira - Chikatto Chika Chika (Agatsu) [Sotarks' 1+2 IQ] | 5.92* | 264pp | 8.25* | 657pp
Will Stetson - Snow Halation (feat. BeasttrollMC) (Sotarks) [Nevo's Extra] | 5.37* | 201pp | 7.47* | 501pp
Vickeblanka - Black Rover (TV Size) (Sotarks) [Extra] | 6.09* | 271pp | 8.67* | 728pp
Parry Gripp - Guinea Pig Bridge (Sotarks) [Guinea Pig Technology] | 5.48* | 201pp | 7.64* | 505pp


# Download:
* [Download .exe file.](https://naitsirk.s-ul.eu/KHaHGflp.exe)
