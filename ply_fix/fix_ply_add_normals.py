from plyfile import PlyData, PlyElement
import numpy as np
import sys
import os

def fix_ply_add_normals(input_path, output_path=None):
    # read PLY
    ply = PlyData.read(input_path)
    vertex = ply['vertex'].data

    # Get headerd
    existing_props = vertex.dtype.names
    vertex_dict = {prop: vertex[prop] for prop in existing_props}

    # Add normal: (0,0,1)
    count = len(vertex)
    vertex_dict['nx'] = np.zeros(count, dtype='f4')
    vertex_dict['ny'] = np.zeros(count, dtype='f4')
    vertex_dict['nz'] = np.ones(count, dtype='f4')  # points to Z axis

    # order props
    target_order = (
        ['x', 'y', 'z'] +
        ['nx', 'ny', 'nz'] +
        ['f_dc_0', 'f_dc_1', 'f_dc_2'] +
        [f'f_rest_{i}' for i in range(45)] +
        ['opacity', 'scale_0', 'scale_1', 'scale_2'] +
        ['rot_0', 'rot_1', 'rot_2', 'rot_3']
    )

    # create new structured array
    fixed_array = np.empty(count, dtype=[(name, 'f4') for name in target_order])
    for name in target_order:
        if name in vertex_dict:
            fixed_array[name] = vertex_dict[name]
        else:
            print(f"Warning: missing expected property {name}, filling with 0.")
            fixed_array[name] = 0.0

    # add new PlyElement
    fixed_vertex = PlyElement.describe(fixed_array, 'vertex')

    # Output
    if output_path is None:
        base = os.path.basename(input_path)
        output_path = os.path.join(os.path.dirname(input_path), f'fixed_{base}')

    # write data
    PlyData([fixed_vertex], text=False).write(output_path)
    print(f"✔ Fixed PLY saved to: {output_path}")

# fix_ply_add_normals("white_flower.ply")

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("Usage: python fix_ply_add_normals.py <input_file.ply>")
    else:
        fix_ply_add_normals(sys.argv[1])
