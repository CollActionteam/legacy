import React from "react";
import logo from "../../../static/assets/logo.svg";
import { Link, useStaticQuery, graphql } from "gatsby";
import Button from "../Button";
import Container from '@material-ui/core/Container';
import styles from "./style.module.scss";

export default () => {
  const data = useStaticQuery(query);

  return (
    <div className={styles.header}>
      <Container>
        <div className={styles.wrapper}>
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
              <li className={styles.navigationItem}>
                <Link to="/donate" className="donationLink">
                  Donate
                </Link>
              </li>
              <li className={styles.navigationItem}>
                <Button to="/login" className="secondary">Login</Button>
              </li>
            </ul>
          </nav>
        </div>
      </Container>
    </div>
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
