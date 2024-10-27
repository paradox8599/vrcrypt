use crate::data::{XMesh, XMeshes};
use serde::Deserialize;
use unsafe_fn::unsafe_str;

#[derive(Deserialize)]
struct CreateRandomMeshesInput {
    meshes: Vec<XMesh>,
    factor: f64,
}

#[unsafe_str]
fn create_random_meshes(input: &str) -> String {
    let save = || {
        std::fs::write("Assets/VRCrypt/meshes.json", input);
    };
    let input = serde_json::from_str::<CreateRandomMeshesInput>(input).map_err(|e| e.to_string());
    match input {
        Ok(input) => {
            let meshes = input
                .meshes
                .iter()
                .map(|m| m.randomized(input.factor))
                .collect::<Vec<_>>();
            XMeshes { meshes }.to_string()
        }
        Err(e) => {
            save();
            return e;
        }
    }
}
