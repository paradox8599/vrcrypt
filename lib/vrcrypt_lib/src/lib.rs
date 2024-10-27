pub mod bind;
pub mod constants;
pub mod data;
pub mod utils;

use data::XMeshList;
use interoptopus::ffi_function;
use std::ffi::{CStr, CString};
use std::os::raw::c_char;
use unsafe_fn::unsafe_str;

#[unsafe_str]
fn read(input: &str) -> String {
    format!("hello {}", input)
}

#[unsafe_str]
fn take_meshes(input: &str) -> String {
    match serde_json::from_str::<XMeshList>(input) {
        Ok(v) => serde_json::to_string(&v).unwrap(),
        Err(e) => e.to_string(),
    }
}
