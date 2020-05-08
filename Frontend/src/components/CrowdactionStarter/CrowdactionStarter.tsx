import React from "react";

import { Avatar } from "@material-ui/core";

import styles from "./CrowdactionStarter.module.scss";

export const CrowdactionStarter = ({user}: any) => {
  
  return (
    <>
      { user &&
        <div className={styles.crowdactionStarter}>
          <div className={styles.avatarContainer}>
            <Avatar className={styles.avatar}>
              {user.firstName?.charAt(0)}
              {user.lastName?.charAt(0)}
            </Avatar>
          </div>
          <h3>{user.fullName}</h3>
          <p className={styles.crowdactionStarterTitle}>Crowdaction starter</p>
        </div>      
      }
    </>
  );
}
