import React from "react";
import styles from "./style.module.scss";
import BackgroundImage from "gatsby-background-image";
import { graphql, StaticQuery } from "gatsby";

interface IOverlayProps {
  children: any;
  photo: string;
}

export const Overlay = ({ children, photo }: IOverlayProps) => (
  <StaticQuery
    query={graphql`
      query {
        allImageSharp {
          edges {
            node {
              fluid(maxWidth: 1920) {
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
      console.log(image.node.fluid);
      const whiteOverlay = "rgba(256, 256, 256, 0.8)";
      const overlay = `linear-gradient(${whiteOverlay}, ${whiteOverlay})`;
      const images = [overlay, image.node.fluid];

      return <BackgroundImage fluid={images}>{children}</BackgroundImage>;
    }}
  />
);
