import React from "react";
import logo from "../../../static/assets/logo.svg";
import { Link, useStaticQuery, graphql } from "gatsby";

import styles from "./style.module.scss";

export default () => {
  const data = useStaticQuery(query);

  return (
    <header className={styles.header}>
      <img alt="CollAction" className={styles.logo} src={logo}></img>
      <nav className={styles.navigation}>
        <ul className={styles.navigationList}>
          {data.site.siteMetadata.menuLinks.map(link => (
            <li key={link.name} className={styles.navigationItem}>
              <Link className={styles.navigationLink} to={link.link}>
                {link.name}
              </Link>
            </li>
          ))}
        </ul>
        <ul className={styles.navigationList}>
          <li class={styles.navigationItem}>
            <button>Donate</button>
          </li>
          <li class={styles.navigationItem}>
            <Link to="/login">Login</Link>
          </li>
        </ul>
      </nav>
    </header>
  );
};

const query = graphql`
  query HeaderQuery {
    site {
      siteMetadata {
        title
        menuLinks {
          name
          link
        }
      }
    }
  }
`;
