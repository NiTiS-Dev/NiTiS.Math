mod output;

use anyhow::{bail, Context, Ok};
use clap::{command, arg};
use output::build_output_pairs;


#[allow(non_upper_case_globals)]
const NiTiSRoot: &str = "..";

fn main() -> anyhow::Result<()> {
    //let matches = command!()
    //    .get_matches();

    let tera = tera::Tera::new("templates/**.cs.tera").context("Tera parsing error(s)")?;

    let repo = git2::Repository::open(NiTiSRoot).context("Failed to open git repo")?;
    let workdir = repo.workdir().unwrap();

    build_output_pairs();

    Ok(())
}
