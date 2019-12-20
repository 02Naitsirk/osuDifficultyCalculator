# What is this?
An experimental project (with bad code) not meant to be taken seriously. It calculates the SR and pp values of osu! beatmaps without using decaying strain, which measures how hard it is to keep up with a pattern. Instead, each object receives a difficulty value based on its distance and time to the previous note. The difficulty values are then added up in a weighted sum, and then converted to SR and pp.

# What can this not do?
All this does is calculate SR and pp. There's no code to calculate pp with a certain accuracy, combo, misses, etc. However, I would recommend the following formula:

`pp = (SS pp) * ARCCOS(1 - 2 * ACCURACY) * SQRT(COMBO / MAX COMBO) * 0.95^(MISSES) / (PI * (2 - ACCURACY)).`

Finally, this system does not have a speed buff.

# Results
Because this system is "strainless", you can expect difficulty spikes to be nerfed, because strain becomes high on difficulty spikes, and  consistent maps to be buffed.

# Download:
* [Download .exe file.](https://naitsirk.s-ul.eu/XHiB50hV.exe)
