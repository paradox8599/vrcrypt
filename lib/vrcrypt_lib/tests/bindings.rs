use interoptopus::{util::NamespaceMappings, Error, Interop};
use interoptopus_backend_csharp::{Config, Generator};

const LIBRARY_NAME: &str = "vrcrypt_lib";

#[test]
fn bindings_csharp() -> Result<(), Error> {
    let inventory = vrcrypt_lib::bind::build_binding_inventory();
    let config = Config {
        class: LIBRARY_NAME.to_string(),
        dll_name: LIBRARY_NAME.to_string(),
        namespace_mappings: NamespaceMappings::new("RustNative"),
        ..Default::default()
    };

    std::fs::create_dir_all("bindings/csharp")?;
    Generator::new(config, inventory).write_file(format!("bindings/csharp/{LIBRARY_NAME}.cs"))?;
    Ok(())
}
