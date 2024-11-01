use proc_macro::TokenStream;
use quote::{format_ident, quote};
use syn::{parse_macro_input, ItemFn};

/// Accepts no arguments
/// Returns Result<String, String>
// #[proc_macro_attribute]
// pub fn ffi_no_args(_attr: TokenStream, item: TokenStream) -> TokenStream {
//     let input_fn = parse_macro_input!(item as ItemFn);
//     let fn_name = &input_fn.sig.ident;
//     let ffi_fn_name = format_ident!("ffi_{}", fn_name);
//     let output = quote! {
//         #input_fn
//
//         /// # Safety
//         #[interoptopus::ffi_function]
//         #[no_mangle]
//         pub unsafe extern "C" fn #ffi_fn_name(input: *const ::std::os::raw::c_char) -> *mut ::std::os::raw::c_char {
//             let output = match #fn_name() {
//                 Ok(v) => format!("v{v}"),
//                 Err(e) => format!("x{e}"),
//             };
//             ::std::ffi::CString::new(output)
//                 .expect(&format!("{}: invalid utf8 output", stringify!(#ffi_fn_name)))
//                 .into_raw()
//         }
//     };
//
//     output.into()
// }

/// Accepts &str
/// Returns Result<String, String>
// #[proc_macro_attribute]
// pub fn ffi_str(_attr: TokenStream, item: TokenStream) -> TokenStream {
//     let input_fn = parse_macro_input!(item as ItemFn);
//     let fn_name = &input_fn.sig.ident;
//     let ffi_fn_name = format_ident!("ffi_{}", fn_name);
//     let output = quote! {
//         #input_fn
//
//         /// # Safety
//         #[interoptopus::ffi_function]
//         #[no_mangle]
//         pub unsafe extern "C" fn #ffi_fn_name(input: *const ::std::os::raw::c_char) -> *mut ::std::os::raw::c_char {
//             let input_str = ::std::ffi::CStr::from_ptr(input).to_str()
//                 .expect(&format!("{}: invalid utf8 input", stringify!(#ffi_fn_name)));
//             let output = match #fn_name(input_str) {
//                 Ok(v) => format!("v{v}"),
//                 Err(e) => format!("x{e}"),
//             };
//             ::std::ffi::CString::new(output)
//                 .expect(&format!("{}: invalid utf8 output", stringify!(#ffi_fn_name)))
//                 .into_raw()
//         }
//     };
//
//     output.into()
// }

#[proc_macro_attribute]
pub fn ffi(_attr: TokenStream, item: TokenStream) -> TokenStream {
    let input_fn = parse_macro_input!(item as ItemFn);
    let fn_name = &input_fn.sig.ident;
    let ffi_fn_name = format_ident!("ffi_{}", fn_name);

    // Count the number of arguments
    let arg_count = input_fn.sig.inputs.len();

    // Generate FFI input parameters
    let ffi_inputs = (0..arg_count).map(|i| {
        let param_name = format_ident!("input_{}", i);
        quote! { #param_name: *const ::std::os::raw::c_char }
    });

    // Generate string conversion for each input
    let input_conversions = (0..arg_count).map(|i| {
        let param_name = format_ident!("input_{}", i);
        quote! {
            let #param_name = ::std::ffi::CStr::from_ptr(#param_name).to_str()
                .expect(&format!("{}: invalid utf8 input for parameter {}", stringify!(#ffi_fn_name), stringify!(#param_name)));
        }
    });

    // Generate function call arguments
    let fn_args = (0..arg_count).map(|i| {
        let param_name = format_ident!("input_{}", i);
        quote! { #param_name }
    });

    // Generate the function call based on number of arguments
    let fn_call = if arg_count == 0 {
        quote! { #fn_name() }
    } else {
        quote! { #fn_name(#(#fn_args),*) }
    };

    let output = quote! {
        #input_fn

        /// # Safety
        #[interoptopus::ffi_function]
        #[no_mangle]
        pub unsafe extern "C" fn #ffi_fn_name(
            #(#ffi_inputs),*
        ) -> *mut ::std::os::raw::c_char {
            #(#input_conversions)*

            let output = match #fn_call {
                Ok(v) => format!("v{v}"),
                Err(e) => format!("x{e}"),
            };

            ::std::ffi::CString::new(output)
                .expect(&format!("{}: invalid utf8 output", stringify!(#ffi_fn_name)))
                .into_raw()
        }
    };

    output.into()
}
