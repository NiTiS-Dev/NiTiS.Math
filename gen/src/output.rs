use std::collections::HashMap;

struct ContextBuilder(tera::Context);

impl ContextBuilder
{
    pub fn new() -> Self
    {
        Self(tera::Context::new())
    }
    fn with_template(mut self, template: &str) -> Self
    {
        self.0.insert("template_path", template);
        self
    }
    fn with_element_type(mut self, name: &str) -> Self
    {
        self.0.insert("element_t", name);
        self
    }
    fn with_dimension(mut self, dim: u32) -> Self
    {
        self.0.insert("dim", &dim);
        self
    }
    pub fn build(self) -> tera::Context
    {
        self.0
    }

    pub fn new_bvec(dim: u32) -> Self
    {
        ContextBuilder::new()
            .with_dimension(dim)
            .with_element_type("bool")
            .with_template("vec.cs.tera")
    }
}

pub fn build_output_pairs() -> HashMap<&'static str, tera::Context>
{
    HashMap::from([
        (
            "BVector2d.cs",
            ContextBuilder::new_bvec(2).build()
        )
    ])
}