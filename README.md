# What is this?
An experimental project (with bad code) not meant to be taken seriously. It calculates the SR and pp values of osu! beatmaps without using decaying strain, a key feature in ppv2 which measures how hard it is to keep up with a pattern. Instead, each object receives a difficulty value based on its distance and time to the previous note. The difficulty values are then added up in a weighted sum, and then converted to SR and pp.

At first, this was just a project to see if I can make the simplest pp system that is still not complete garbage, but it quickly became more convoluted.

# What can this not do?
All this does is calculate SR and pp. There's no code to calculate pp with a certain accuracy, combo, and miss count. However, I would recommend the following formula:

`pp = (SS pp) * ARCCOS(1 - 2 * ACCURACY) * SQRT(COMBO / MAX COMBO) * 0.95^(MISSES) / (PI * (2 - ACCURACY)).`

# Results
Because this system is "strainless", you can expect difficulty spikes to be nerfed (because strain becomes very high on difficulty spikes). Examples of this are Harumachi Clover, Sotarks's difficulty on supercell - Hero, You Suck at Love, and so on.

You can also expect tough patterns that last very briefly to be buffed too. Maps like Atomosphere, Bonzi's difficulty on Polygon, AXION, and others receive significant buffs for that exact reason. The spaced triples in Atomosphere, the streamjumps on Polygon, and other difficult patterns last too briefly to be properly accounted for in ppv2.

Sadly, maps like God Speed, A Hope in Hell, Porky of Porky of Porky, etc. are extremely overweighted because of how "consistent" they are. The problems are even worse on DT. Previously, maps such as Gengaozo-foon, Burnt Rice, and Pitch Test were ridiculously overweighted, but I managed to rectify that issue. However, the former maps are still broken and would probably only be fixed with a system that uses strain. Fortunately, most maps that I have tested do not suffer from this issue.

The angle buff in this system is different than ppv2's. Instead of buffing a specific angle (like 180 degrees), it buffs angle changes. This might be a better measure of aim control, seeing how Mission ASCII +DT and other similar maps get notable buffs, and it doesn't overbuff any maps.

In addition, speed is, in some rare cases, broken. Any pattern that has almost no spacing will be underweighted, especially stacked streams. Maps like Hidamari no Uta +DT are underweighted for that reason. Luckily, if a map has just a bit of spacing on those patterns, the pattern will not be very underweighted. For example, silver temple +DT, ICARUS +DT, Everything will freeze [Lunatic] +DT, and other similar maps actually get a massive buff, even though the streams are close together. Yes, this system does contain a speed buff starting at 240 bpm, capped at 400 bpm.

Sliders are accounted for, sort of. I'm a bit too lazy to use green lines, so I approximate slider duration by subtracting the current note's timestamp from the next note's timestamp. Maps like Notch Hell, KAEDE (still underrated), Railgun Roulette VIP, and others recieve a significant buff from slider difficulty.

Here is a table of almost all the maps mentioned in this section:

Map|Old SR|New SR|Old SS|New SS|
---|---|---|---|---|
Will Stetson - Harumachi Clover (Swing Arrangement) (Will Stetson) [Fiery's Extreme]|5.82*|5.02*||223pp|157pp|
supercell - Hero (Gottagof4st) [Sotarks' Wish]|6.20*|5.83*|290pp|249pp|
Simple Plan - You Suck At Love (Speed Up Ver.) (Reform) [Extra]|5.92*|5.70*|267pp|239pp|
Camellia - Exit This Earth's Atomosphere (rrtyui) [Evolution]|7.24*|8.37*|503pp|723pp|
Sota Fujimori - polygon (Kaifin) [Bonzi's Ultra]|7.48*|8.02*|562pp|590pp|
sakuzyo - AXION (Flower) [AXION_REBORN]|6.96*|7.57*|412pp|486pp|
Okui Masami - God Speed (ykcarrot) [Insane]|5.19*|5.84*|218pp|278pp|
Glamour of the Kill - A Hope in Hell (ykcarrot) [Hopeless]|5.66*|6.60*|268pp|384pp|
REDALiCE - Porky of Porky of Porky of Porky (K-S-O) [Ultimate Chimera]|4.92*|6.20*|180pp|319pp|
Traktion - Mission ASCII (galvenize) [Another] +DT|7.76*|8.02*|697pp|748pp|
Yuyoyuppe - Hidamari no Uta (Seto Kousuke) [Expert] +DT|8.28*|6.18*|749pp|387pp|
Eguchi Takahiro - silver temple (LKs) [Insane] +DT|7.11*|8.00*|494pp|619pp|
Eagle - ICARUS (Muya) [Another] +DT|8.00*|8.60*|621pp|762pp|
UNDEAD CORPORATION - Everything will freeze (Ekoro) [Lunatic]|7.90*|8.86*|647pp|854pp|
Halozy - Kikoku Doukoku Jigokuraku (Hollow Wings) [Notch Hell]|5.31*|6.10*|216pp|297pp|
Ocelot - KAEDE (Hollow Wings) [EX EX]|5.61*|6.31*|242pp|296pp|
Camellia vs Akira Complex - Railgun Roulette (VIP) (NeilPerry) [Neil x Sharu, Syzygy]|6.49*|7.68*|387pp|583pp|
