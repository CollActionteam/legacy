import React, {useState} from "react";

import styles from "./Faq.module.scss";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import { useAnalytics } from "../../providers/AnalyticsProvider";

interface IFaqProps {
    children: any;
    title: string;
    collapsed: boolean;
    faqId: string;
}

export const Faq = ({ children, title, collapsed, faqId }: IFaqProps) => {
    const [ isCollapsed, setCollapsed ] = useState(collapsed);
    const { sendUserEvent } = useAnalytics();
    return (
        <div className={styles.faq}>
            <h3 onClick={() => { setCollapsed(!isCollapsed); sendUserEvent(false, 'faq', 'click', faqId, null); }}>
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
