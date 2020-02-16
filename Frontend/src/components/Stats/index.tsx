import React from "react";
import Grid from "@material-ui/core/Grid";
import Drop from "../../../static/assets/logo.svg";
import styles from "./style.module.scss";
import { graphql, StaticQuery } from "gatsby";

export default () => (
  <StaticQuery
    query={graphql`
      stats: allMarkdownRemark(
        filter: { frontmatter: { type: { eq: "stats" } } }
        sort: { fields: frontmatter___sequence }
      ) {
        nodes {
          html
          frontmatter {
            name
          }
        }
      }
    }
    `}
    render={data => {
      const stats = data.stats.nodes;
      return (
        <Grid container className={styles.main}>
          {stats.map((stat, index) => (
            <Grid key={index} item xs={12} md={4} className={styles.stat}>
              <h2>{stat.frontmatter.name}</h2>
              <span dangerouslySetInnerHTML={{ __html: stat.html }}></span>
              <img src={Drop} />
            </Grid>
          ))}
        </Grid>
      );
    }}
  />
);
