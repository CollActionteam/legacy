import React from "react";
import { Link } from "react-router-dom";
import { HashLink } from "react-router-hash-link";
import { siteData } from "../../api/site";

import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import logo from "../../assets/svg/logo.svg";

import SocialMedia from "../SocialMedia/SocialMedia";
import NewsletterSignup from "../NewsletterSignup/NewsletterSignup";

import styles from "./Footer.module.scss";
import { gql, useQuery } from "@apollo/client";
import { Alert } from "../Alert/Alert";

const GET_MAILCHIMP_LISTID = gql`
  query {
    miscellaneous {
      mailChimpNewsletterListId
    }
  }
`;

export default () => {
  const { data: mailchimpData, error } = useQuery(GET_MAILCHIMP_LISTID);
  const mailChimpId = mailchimpData?.miscellaneous?.mailChimpNewsletterListId ?? "";
  return <div className={styles.footer}>
    <Container>
      <Alert type="error" text={error?.message} />
      <Link to="/">
        <img alt="CollAction" className={styles.logo} src={logo}></img>
      </Link>
      <Grid container>
        <Grid item xs={12} md={4}>
          <div className={styles.description}>
            Any questions, comments, or would you like to work together?
            Awesome! Email us at{" "}
            <a href="mailto:collactionteam@gmail.com">
              collactionteam@gmail.com
            </a>
            .<br />
            Would you like to get an occasional update on CollAction and
            crowdacting? Sign up for our newsletter (to your right)! We will be
            careful with your data, see our <Link to="/privacy-policy">Privacy Policy</Link>.
          </div>
        </Grid>
        <Grid item xs={12} sm={6} md={5}>
          <nav className={styles.navigation}>
            <ul className={styles.navigationList}>
              {siteData.footerLinks.map((link: any, index: number) => (
                <li key={index} className={styles.navigationItem}>
                  <HashLink className={styles.navigationLink} to={link.link}>
                    {link.name}
                  </HashLink>
                </li>
              ))}
            </ul>
          </nav>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <h5 className={styles.title}>Follow us</h5>
          <SocialMedia socialMedia={siteData.socialMedia} />
          <h5 className={styles.title}>Newsletter</h5>
          <NewsletterSignup mailchimpListId={mailChimpId} />
        </Grid>
      </Grid>
    </Container>
  </div>;
};
