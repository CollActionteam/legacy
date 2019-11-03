import React from "react";
import styles from "./style.module.scss";
import { Grid } from "@material-ui/core";
import BackgroundImage from "gatsby-background-image";
import { graphql, StaticQuery } from "gatsby";

export const Banner = ({ children, photo, dots = false }) => (
  <StaticQuery
    query={graphql`
      query {
        allImageSharp {
          edges {
            node {
              fluid(maxWidth: 1900) {
                ...GatsbyImageSharpFluid
                originalName
              }
            }
          }
        }
      }
    `}
    render={data => {
      const image = data.allImageSharp.edges.find(
        edge => edge.node.fluid.originalName === photo
      );
      return (
        <BackgroundImage fluid={image.node.fluid}>
          {dots ? <div className={styles.dots}></div> : null}
          <Grid container>{children}</Grid>
        </BackgroundImage>
      );
    }}
  />
);
