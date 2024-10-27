#[macro_export]
macro_rules! build_inventory_with {
    ($($fn:ident),+$(,)*) => {
        InventoryBuilder::new()
            $(.register(function!($fn)))+
            .inventory()
    };
}
