pub mod bind;
pub mod cmd;
pub mod constants;
pub mod data;
pub mod utils;

use unsafe_fn::unsafe_str;

#[unsafe_str]
fn read(input: &str) -> String {
    format!("hello {}", input)
}
