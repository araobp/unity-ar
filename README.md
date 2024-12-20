# AR building blocks

I have been working on AR apps since Y2021. This repo is for sorting out AR building blocks that I have developed in those AR apps.

Requirements:
- Unity
- ARFoundation and ARKit/ARCore

## Building blocks

### EDM (LiDAR-based)

Note: this building block requires iPhone/iPad equipped with LiDAR.

EDM (Electronic Distance Measurement) in the surveying terminology is useful for AR applications as well.

Last year I saw this AR device "[SiteVision](https://sitevision.trimble.com)" at a trade show in Tokyo. The device is also equipped with Lazer-based EDM.

iPad Pro and iPhone 12 Pro are equipped with LiDAR. I have found that EDM based on the LiDAR is very useful when I am placing an AR anchor on a wall.

<img src="doc/EDM_test.PNG" width=200>

=> [code](./unity/EDM)

### EDM (PointCloud-based)

Most of smartphone models in the market are not equipped with LiDAR. We use point cloud for distance measurement instead of LiDAR.

=> [code](./unity/EDM2)

### World origin visualization

This app is just to visualize the origin on the world coordinates.

<img src="doc/WorldOrigin.PNG" width=200>

=> [code](./unity/WorldOrigin)

### QR code reader

This app uses ARCameraBackground for recognizing a QR code with ZXing library.

<img src="doc/QRCodeReader.PNG" width=200>

=> [code](./unity/QRCodeReader)

### Rendering IFC BIM in AR app (URP)

This is to explane how IFC-format BIM can be rendered in my original AR app.

I downloaded "FZK Haus" and "Azuma House" from the following BIM archive:
http://openifcmodel.cs.auckland.ac.nz

Then I modified the original BIM with Blender (with BlenderBIM add-on) for my AR app:
- modified some parts to make them look better
- added CC0Texture to some faces
- added a piano chair
- etc
 
<img src="doc/AzumaHouse.jpg" width=250>

=> [Demo video on YouTube](https://youtu.be/BFTbxZobyvY)

=> [code](./unity/ObjectPlacement)

### Dancing Mixamo characters

This is a typical AR app, but it uses the EDM feature above for placing an object at a specific location.

<img src="doc/DanceAim.jpg" width=200>

<img src="doc/Dance.jpg" width=200>

=> [code](./unity/Dance)

### Multiple AR markers (URP)

This app supports multiple AR markers.

<img src="doc/Chair.jpg" width=200>

=> [code](./unity/ARMarkers)

### VR Theater (URP)

This app is to display 240-degree 16K panorama pictures taken with iPhone SE Camera App. iPad mini is the best device to enjoy the high-resolution panorama pictures with this AR/VR app. Just save you panorama pictures in the folder "Resources/Panorama" and make the settings below before starting the app.

<img src="doc/PanoramaSettings.jpg" width=400>

I created the following shader graph for displaying a panorama picture (Texture2D) on the 240-degree screen.

<img src="doc/VRTheaterShaderGraph.png" width=400>

And the screenshots of this AR app on my iPad mini.

<img src="doc/VRTheater1.jpg" width=400>

<img src="doc/VRTheater2.jpg" width=400>

=> [code](./unity/VRTheater)

Note: I have disabled the "Auto Focus" option in Camera Manager to stabilize the screen.

### Converting PDF into transparent PNG inverting black and white

[This Python3 script](./python/PDF_converter_for_AR.py) is to convert a PDF document into a transparent PNG image for AR applications:

```
$ PDF_converter_for_AR.py -i floorplan_sample.pdf
```

<img src="./doc/floorplan_sample.jpg" width=600>
