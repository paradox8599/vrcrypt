#[macro_export]
macro_rules! build_inventory_with {
    ($($fn:ident),+$(,)*) => {
        interoptopus::InventoryBuilder::new()
            $(.register(interoptopus::function!($fn)))+
            .inventory()
    };
}

/// # Safety
#[interoptopus::ffi_function]
#[no_mangle]
pub unsafe extern "C" fn free_str(input: *mut ::std::os::raw::c_char) {
    if !input.is_null() {
        let _ = std::ffi::CStr::from_ptr(input);
    }
}

pub type FResult = Result<String, String>;
