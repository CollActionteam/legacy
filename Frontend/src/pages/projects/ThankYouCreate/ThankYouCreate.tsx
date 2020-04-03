import React from "react";
import { Overlay } from "../../../components/Overlay/Overlay";
import { Section } from "../../../components/Section/Section";

import styles from "./ThankYouCreate.module.scss";

const ThankYouCreatePage = ({ data } : any) => {
  const name = "ProjectName";
  const photos = {
      thankyoucreatephoto: ""
  };

  return (
    <Overlay photo={photos.thankyoucreatephoto}>
    <Section>
        <h1 className={styles.thankYouOverlayTitle}>Awesome!</h1>
        <h2 className={styles.thankYouOverlaySubtitle}>
        Thank you for submitting {name}!
        </h2>
        The CollAction Team will review your project as soon as possible.
        <br />
        If it meets all the CollAction criteria we’ll publish the project on
        the website and will let you know, so you can start promoting it!
        <br />
        If we have any additional questions or comments, we’ll reach out to
        you by email.
        <br />
        Thanks!
    </Section>
    </Overlay>
  );
};

export default ThankYouCreatePage;
