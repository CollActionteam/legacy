import React from "react";
import { IconButton } from "../Button/Button";

import styles from "./SocialMedia.module.scss";
import { useAnalytics } from "../../providers/AnalyticsProvider";

export default ({ socialMedia } : any) => {
  const { sendUserEvent } = useAnalytics();
  return (
    <ul className={styles.list}>
      {socialMedia.map((item: any, index: number) => (
        <li key={index} className={styles.listItem}>
          <IconButton aria-label={`Share on ${item.name}`} onClick={() => { sendUserEvent(false, 'share', 'share on social media', item.name, null); window.open(item.url, '_blank'); }} icon={["fab", item.icon]} />
        </li>
      ))}
    </ul>
  );
};