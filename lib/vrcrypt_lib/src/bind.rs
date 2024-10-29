use crate::{cmd::ffi_create_random_meshes, utils::free_str};

pub fn build_binding_inventory() -> interoptopus::Inventory {
    crate::build_inventory_with!(free_str, ffi_create_random_meshes)
}
