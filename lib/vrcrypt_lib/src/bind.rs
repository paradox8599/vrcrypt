use crate::{build_inventory_with, cmd::unsafe_create_random_meshes, unsafe_free_str, unsafe_read};
use interoptopus::{function, Inventory, InventoryBuilder};

pub fn build_binding_inventory() -> Inventory {
    build_inventory_with!(unsafe_free_str, unsafe_read, unsafe_create_random_meshes,)
}
