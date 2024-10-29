use crate::{data::XMesh, utils::FResult};

#[derive(serde::Deserialize)]
struct Input {
    meshes: Vec<XMesh>,
    factor: f64,
}

#[derive(serde::Serialize)]
struct Output {
    meshes: Vec<XMesh>,
}

#[ffi_fn::ffi]
fn create_random_meshes(input: &str) -> FResult {
    let input = serde_json::from_str::<Input>(input);

    match input {
        Ok(input) => {
            let meshes = input
                .meshes
                .iter()
                .map(|m| m.randomized(input.factor))
                .collect::<Vec<_>>();

            Ok(serde_json::to_string(&Output { meshes })
                .expect("create_random_meshes: ouptut serialization failed"))
        }

        Err(e) => Err(e.to_string()),
    }
}
