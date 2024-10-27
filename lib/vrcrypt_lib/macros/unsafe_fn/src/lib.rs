use proc_macro::TokenStream;
use quote::{format_ident, quote};
use syn::{parse_macro_input, ItemFn};

#[proc_macro_attribute]
pub fn unsafe_str(_attr: TokenStream, item: TokenStream) -> TokenStream {
    // Parse the input function
    let input_fn = parse_macro_input!(item as ItemFn);

    // Get the function name and create a new name for the wrapper function
    let fn_name = &input_fn.sig.ident;
    let wrapper_fn_name = format_ident!("unsafe_{}", fn_name);

    // Generate the new function
    let output = quote! {
        #input_fn

        /// # Safety
        #[interoptopus::ffi_function]
        #[no_mangle]
        pub unsafe extern "C" fn #wrapper_fn_name(input: *const ::std::os::raw::c_char) -> *mut ::std::os::raw::c_char {
            let input_str = ::std::ffi::CStr::from_ptr(input).to_str().unwrap_or("");
            let output = #fn_name(input_str);
            ::std::ffi::CString::new(output).unwrap().into_raw()
        }
    };

    output.into()
}
