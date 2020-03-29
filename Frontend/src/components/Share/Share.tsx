import React from "react";
import { useTranslation } from 'react-i18next';
import { siteData } from "../../api/site";

import SocialMedia from "../SocialMedia/SocialMedia";
import styles from "./Share.module.scss";
import { Grid } from "@material-ui/core";

export default () => {
  const { t } = useTranslation();

  const socialMedia = siteData.socialMedia
    .filter((platform: any) => platform.shareUrl)
    .map((platform: any) => {
      return {
        ...platform,
        url: platform.shareUrl("http://www.collaction.org")
      }
    });

  return (
    <Grid container justify="center">
      <Grid item xs={12} sm={8}>
        <div className={styles.container}>
          <span className={styles.title}>{t('home.share.title')}</span>
          <SocialMedia socialMedia={socialMedia} />
        </div>
      </Grid>
    </Grid>
  );
};