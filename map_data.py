#!/usr/bin/env python

import json

data = json.load(open('Tiles/Level1.json'))

layer = data['layers'][0]

width = layer['width']
height = layer['height']
layer_data = layer['data']

with open('Assets/MapData.cs', 'w') as out:
    out.write("""
public class MapData {
    public const int width = %s;
    public const int height = %s;
    public static byte[] tileData = {%s};
}""" % (width, height, ','.join(str((v - 1) & 0xff) for v in layer_data))
)
