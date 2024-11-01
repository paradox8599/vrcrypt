use crate::{
    mesh::{ffi_decrypt_v1, ffi_encrypt_v1, ffi_randomize},
    utils::free_str,
};

pub fn build_binding_inventory() -> interoptopus::Inventory {
    crate::build_inventory_with!(free_str, ffi_randomize, ffi_encrypt_v1, ffi_decrypt_v1,)
}
