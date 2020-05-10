import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

import styles from "./NewsletterSignup.module.scss";
import { useSettings } from "../../providers/SettingsProvider";

export default () => {
  const [ email, setEmail ] = useState("");
  const { mailChimpAccount, mailChimpNewsletterListId, mailChimpServer, mailChimpUserId } = useSettings();

  return <form action={`//${mailChimpAccount}.${mailChimpServer}.list-manage.com/subscribe/post?u=${mailChimpUserId}&id=${mailChimpNewsletterListId}`}
               method="post"
               target="_blank">
    <div className={styles.wrapper}>
      <input
        type="email"
        placeholder="Fill in your e-mail"
        aria-label="E-Mail Address"
        name="EMAIL"
        value={email}
        onChange={(ev) => setEmail(ev.target.value)} />
      <div className={styles.hidden} aria-hidden="true">
        <input
          type="text"
          place-holder="E-Mail"
          name={`b_${mailChimpUserId}_${mailChimpNewsletterListId}`}
          value=""
          tabIndex={-1}
          readOnly={true} />
      </div>
      <button
        name="subscribe"
        aria-label="Subscribe Newsletter"
        className={styles.submit}
        disabled={!email}>
        <div className={styles.submitIcon}>
          <FontAwesomeIcon icon={["far", "envelope"]} size="sm" />
        </div>
        {email ? (
          <span className={styles.submitLabel}>Sign up</span>
        ) : null}
      </button>
    </div>
  </form> ;
}
