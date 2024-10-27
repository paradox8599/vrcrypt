use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct Vector2 {
    pub x: f64,
    pub y: f64,
}

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct Vector3 {
    pub x: f64,
    pub y: f64,
    pub z: f64,
}

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct Vector4 {
    pub x: f64,
    pub y: f64,
    pub z: f64,
    pub w: f64,
}

#[derive(Serialize, Deserialize, Debug, Default)]
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

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct Color {
    pub r: f64,
    pub g: f64,
    pub b: f64,
    pub a: f64,
}

#[derive(Serialize, Deserialize, Debug, Default)]
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

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct Bounds {
    pub center: Vector3,
    pub extents: Vector3,
}

#[derive(Serialize, Deserialize, Debug, Default)]
pub enum MeshTolopogy {
    #[default]
    Triangles = 0,
    Quads = 2,
    Lines = 3,
    LineStrip = 4,
    Points = 5,
}

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct SubMeshDescriptor {
    pub bounds: Bounds,
    pub topology: MeshTolopogy,
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

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct XBlendShape {
    pub name: String,
    #[serde(rename = "deltaVertices")]
    pub delta_vertices: Vec<Vector3>,
    #[serde(rename = "deltaNormals")]
    pub delta_normals: Vec<Vector3>,
    #[serde(rename = "deltaTangents")]
    pub delta_tangents: Vec<Vector3>,
    #[serde(rename = "frameWeight")]
    pub frame_weight: f64,
    #[serde(rename = "frameCount")]
    pub frame_count: i32,
}

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct XMesh {
    pub path: String,
    pub name: String,
    pub vertices: Vec<Vector3>,
    pub triangles: Vec<i32>,
    pub normals: Vec<Vector3>,
    pub tangents: Vec<Vector4>,
    pub colors: Vec<Color>,
    #[serde(rename = "boneWeights")]
    pub bone_weights: Vec<BoneWeight>,
    pub bindposes: Vec<Matrix4x4>,
    #[serde(rename = "boundsCenter")]
    pub bounds_center: Vector3,
    #[serde(rename = "boundsExtents")]
    pub bounds_extents: Vector3,
    #[serde(rename = "subMeshes")]
    pub sub_meshes: Option<Vec<SubMeshDescriptor>>,
    pub uvs: Option<Vec<Vec<Vector2>>>,
    #[serde(rename = "blendShapes")]
    pub blend_shapes: Vec<XBlendShape>,
}

#[derive(Serialize, Deserialize, Debug, Default)]
pub struct XMeshList {
    pub meshes: Vec<XMesh>,
}
