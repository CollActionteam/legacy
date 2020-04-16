import React, {useState} from "react";

import styles from "./Faq.module.scss";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";

export const Faq = ({ children, title, collapsed }: any) => {
    const [isCollapsed, setCollapsed] = useState(collapsed);
    return (
        <div className={styles.faq}>
            <h3 onClick={() => setCollapsed(!isCollapsed)}>
                {title}
                <FontAwesomeIcon
                    icon={isCollapsed ? "plus" : "minus"}
                    className={styles.icon} />
            </h3>
            <div style={{display: isCollapsed ? "none" : "block"}}>
            {children}
            </div>
        </div>
    );
}
