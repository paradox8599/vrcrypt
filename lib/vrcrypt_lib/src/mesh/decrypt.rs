use crate::{data::XMesh, utils::FResult};

#[derive(serde::Deserialize)]
struct Input {
    meshes: Vec<XMesh>,
    key: String,
    factor: f64,
}

#[derive(serde::Serialize)]
struct Output {
    meshes: Vec<XMesh>,
}

#[ffi_fn::ffi]
fn decrypt_v1(input: &str) -> FResult {
    let input = serde_json::from_str::<Input>(input);

    match input {
        Ok(input) => {
            let meshes = input
                .meshes
                .iter()
                .map(|m| m.decrypted_v1(&input.key, input.factor))
                .collect::<Vec<_>>();

            Ok(serde_json::to_string(&Output { meshes })
                .expect("decrypt: ouptut serialization failed"))
        }

        Err(e) => Err(e.to_string()),
    }
}
