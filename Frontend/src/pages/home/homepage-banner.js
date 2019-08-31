import React from "react";

import styles from './style.module.scss';
import { Banner } from "../../components/Banner";
import { CallToAction } from "./call-to-action";

export const HomepageBanner = ({ photo, title }) => {
  return (
    <Banner photo={ photo }>
      <div className={ styles.banner }>
        <CallToAction title={ title }></CallToAction>
      </div>
    </Banner>
  )
};
