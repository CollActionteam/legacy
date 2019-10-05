import React from "react";
import logo from "../../../static/assets/logo.svg";
import { Link, useStaticQuery, graphql } from "gatsby";
import Container from "@material-ui/core/Container";
import styles from "./style.module.scss";
import Navigation from "../Navigation";

export default () => {
  const data = useStaticQuery(query);

  return (
    <div className={styles.header}>
      <Container>
        <div className={styles.wrapper}>
          <Link to="/">
            <img alt="CollAction" className={styles.logo} src={logo}></img>
          </Link>
          <Navigation
            items={data.site.siteMetadata.menuLinks.filter(
              link => !!link.showInPrimaryNavigation
            )}
          />
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
          showInPrimaryNavigation
        }
      }
    }
  }
`;
