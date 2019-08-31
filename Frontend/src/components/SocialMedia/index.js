import React from "react";
import { useStaticQuery } from "gatsby";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import styles from "./style.module.scss";
import { IconButton } from "../Button";

export default () => {
  const data = useStaticQuery(query);

  return (
    <ul className={styles.list}>
      {data.site.siteMetadata.socialMedia.map(item => (
        <li className={styles.listItem}>
          <IconButton to={item.url}>
            <FontAwesomeIcon icon={['fab', item.icon]}></FontAwesomeIcon>
          </IconButton>
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
