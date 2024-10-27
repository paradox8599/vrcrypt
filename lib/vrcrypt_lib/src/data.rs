use std::fmt::Display;

use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct Vector2 {
    pub x: f64,
    pub y: f64,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct Vector3 {
    pub x: f64,
    pub y: f64,
    pub z: f64,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct Vector4 {
    pub x: f64,
    pub y: f64,
    pub z: f64,
    pub w: f64,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct Matrix4x4 {
    pub e00: f64,
    pub e10: f64,
    pub e20: f64,
    pub e30: f64,
    pub e01: f64,
    pub e11: f64,
    pub e21: f64,
    pub e31: f64,
    pub e02: f64,
    pub e12: f64,
    pub e22: f64,
    pub e32: f64,
    pub e03: f64,
    pub e13: f64,
    pub e23: f64,
    pub e33: f64,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct Color {
    pub r: f64,
    pub g: f64,
    pub b: f64,
    pub a: f64,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct BoneWeight {
    #[serde(rename = "m_BoneIndex0")]
    pub bone_index_0: i32,
    #[serde(rename = "m_BoneIndex1")]
    pub bone_index_1: i32,
    #[serde(rename = "m_BoneIndex2")]
    pub bone_index_2: i32,
    #[serde(rename = "m_BoneIndex3")]
    pub bone_index_3: i32,
    #[serde(rename = "m_Weight0")]
    pub weight_0: f64,
    #[serde(rename = "m_Weight1")]
    pub weight_1: f64,
    #[serde(rename = "m_Weight2")]
    pub weight_2: f64,
    #[serde(rename = "m_Weight3")]
    pub weight_3: f64,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct Bounds {
    pub center: Vector3,
    pub extents: Vector3,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub enum MeshTolopogy {
    #[default]
    Triangles = 0,
    Quads = 2,
    Lines = 3,
    LineStrip = 4,
    Points = 5,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone, Copy)]
pub struct SubMeshDescriptor {
    pub bounds: Bounds,
    pub topology: i32,
    #[serde(rename = "indexStart")]
    pub index_start: i32,
    #[serde(rename = "indexCount")]
    pub index_count: i32,
    #[serde(rename = "baseVertex")]
    pub base_vertex: i32,
    #[serde(rename = "firstVertex")]
    pub first_vertex: i32,
    #[serde(rename = "vertexCount")]
    pub vertex_count: i32,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct XBlendShape {
    pub name: String,
    pub frames: Vec<XBlendShapeFrame>,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct XBlendShapeFrame {
    pub weight: f32,
    #[serde(rename = "deltaVertices")]
    pub delta_vertices: Vec<Vector3>,
    #[serde(rename = "deltaNormals")]
    pub delta_normals: Vec<Vector3>,
    #[serde(rename = "deltaTangents")]
    pub delta_tangents: Vec<Vector3>,
}

#[derive(Serialize, Deserialize, Debug, Default, Clone)]
pub struct XMesh {
    pub path: String,
    pub name: String,
    pub vertices: Vec<Vector3>,
    pub triangles: Vec<i32>,
    pub tangents: Vec<Vector4>,
    pub colors: Vec<Color>,
    #[serde(rename = "boneWeights")]
    pub bone_weights: Vec<BoneWeight>,
    pub bindposes: Vec<Matrix4x4>,
    #[serde(rename = "blendShapes")]
    pub blend_shapes: Vec<XBlendShape>,
    #[serde(rename = "subMeshes")]
    pub sub_meshes: Vec<SubMeshDescriptor>,
    pub uv: Vec<Vector2>,
    pub uv2: Vec<Vector2>,
    pub uv3: Vec<Vector2>,
    pub uv4: Vec<Vector2>,
    pub uv5: Vec<Vector2>,
    pub uv6: Vec<Vector2>,
    pub uv7: Vec<Vector2>,
    pub uv8: Vec<Vector2>,
}

impl XMesh {
    pub fn randomized(&self, factor: f64) -> XMesh {
        let mut mesh = self.clone();
        for v in mesh.vertices.iter_mut() {
            *v = Vector3 {
                x: v.x + (rand::random::<f64>() - 0.5) * factor,
                y: v.y + (rand::random::<f64>() - 0.5) * factor,
                z: v.z + (rand::random::<f64>() - 0.5) * factor,
            };
        }
        mesh
    }
}

#[derive(Serialize, Deserialize, Debug, Default, Clone)]
pub struct XMeshes {
    pub meshes: Vec<XMesh>,
}

impl Display for XMeshes {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{}", serde_json::to_string(&self).unwrap())
    }
}
