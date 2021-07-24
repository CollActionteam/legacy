import React from "react";
import { HashLink } from "react-router-hash-link";
import { siteData } from "../../api/site";

import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";

import SocialMedia from "../SocialMedia/SocialMedia";
import NewsletterSignup from "../NewsletterSignup/NewsletterSignup";

import styles from "./Footer.module.scss";

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return <div className={styles.footer}>
    <Container>
      <Grid container>
        {siteData.footerLinks.map((column: { title: string, links: any[] }) => (
          <Grid key={column.title} item xs={4} sm={3}>
            <h5 className={styles.title}>{column.title}</h5>
            <nav className={styles.navigation}>
              <ul className={styles.navigationList}>
                {column.links.map((link: any, index: number) => (
                  <li key={index} className={styles.navigationItem}>
                    <HashLink className={styles.navigationLink} to={link.link}>
                      {link.name}
                    </HashLink>
                  </li>
                ))}
              </ul>
            </nav>
          </Grid>
        ))}
        <Grid item xs={12} sm={3}>
          <h5 className={styles.title}>Contact</h5>
          <div className={styles.description}>
            Any questions, comments, or would you like to work together?
            Awesome! Email us at{" "}
            <a href="mailto:hello@collaction.org">
              hello@collaction.org
            </a>
          </div>
          <h5 className={styles.title}>Newsletter</h5>
          <NewsletterSignup />
        </Grid>
      </Grid>
      <Grid container>
        <Grid item xs={12}>
          <div className={styles.copyright}>
            © {currentYear} CollAction
            <SocialMedia socialMedia={siteData.socialMedia} />
          </div>
        </Grid>
      </Grid>
    </Container>
  </div>;
};

export default Footer;