use crate::{build_inventory_with, unsafe_read, unsafe_take_meshes};
use interoptopus::{function, Inventory, InventoryBuilder};

pub fn build_binding_inventory() -> Inventory {
    build_inventory_with!(unsafe_read, unsafe_take_meshes)
}
