# 3D Gaussian Splatting VR Editor

This is a virtual reality (VR) editor for inspecting and cleaning 3D Gaussian Splatting (3DGS) models by selecting and removing floating artifacts. The tool provides intuitive 3D navigation, brush-based volumetric selection, and real-time manipulation of splats within a VR environment.

### Overview
3D Gaussian Splatting (3DGS) offers real-time rendering for 3D reconstruction using Gaussian ellipsoids. While powerful, 3DGS often produces floating artifacts due to motion, lighting inconsistency, or occlusions. This VR tool allows users to:

- Navigate 3DGS models in VR  
- Select and delete floating splats using a volumetric brush  
- Adjust splat size 
- Insert/delete Cutboxes during runtime.


## 🕹️ VR Interaction Design

| Feature         | Description                                                                 |
|----------------|-----------------------------------------------------------------------------|
| Travel          | Hold left trigger → move controller → navigate directionally               |
| Rotate          |Hold both triggers → Left thumbstick (horizontal model rotation)       |
| Scale           | Hold both triggers → increase/decrease controller distance                 |
| Cutbox          | Add/delete cutbox via wrist menu                                           |
| Clean Brush     | Enable edit mode → use right controller brush → select(or pressing trigger to deselect splats)|
| Delete Splats   | Press Delete in wrist menu (⚠ irreversible during session)                 |
| Resize Splats   | Use slider in wrist menu for better visualization & performance            |



### Related
This project is based on https://github.com/roth-hex-lab/Multi-Layer-Gaussian-Splatting-for-Immersive-Anatomy-Visualization
which provides code for creating multi-layer Gaussian Splatting representations.

Unity Rendering code at: https://github.com/roth-hex-lab/Multi-Layer-Anatomy-GS-Unity-Rendering

