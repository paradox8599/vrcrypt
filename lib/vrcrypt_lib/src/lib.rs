pub mod bind;
pub mod cmd;
pub mod constants;
pub mod data;
pub mod utils;

use std::ffi::CStr;

use unsafe_fn::unsafe_str;

#[unsafe_str]
fn read(input: &str) -> String {
    format!("hello {}", input)
}

/// # Safety
#[interoptopus::ffi_function]
#[no_mangle]
pub unsafe extern "C" fn unsafe_free_str(input: *mut ::std::os::raw::c_char) {
    if !input.is_null() {
        let _ = CStr::from_ptr(input);
    }
}
