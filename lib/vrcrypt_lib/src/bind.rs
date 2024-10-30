use crate::{mesh::ffi_randomize, utils::free_str};

pub fn build_binding_inventory() -> interoptopus::Inventory {
    crate::build_inventory_with!(free_str, ffi_randomize)
}
