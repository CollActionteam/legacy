import React from "react";
import { useStaticQuery, graphql } from "gatsby";
import styles from "./style.module.scss";
import { IconButton } from "../Button";

export default () => {
  const data = useStaticQuery(query);

  return (
    <ul className={styles.list}>
      {data.site.siteMetadata.socialMedia.map((item, index) => (
        <li key={index} className={styles.listItem}>
          <IconButton url={item.url} icon={["fab", item.icon]} />
        </li>
      ))}
    </ul>
  );
};

const query = graphql`
  query SocialMediaQuery {
    site {
      siteMetadata {
        socialMedia {
          name
          url
          icon
        }
      }
    }
  }
`;
