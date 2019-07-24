import React from "react";
import { Link } from "gatsby";

const Header = ({ menuLinks }) => (
    <header>
      <div>
        <div
          style={{
            width: "100%",
            display: "flex",
            justifyItems: "space-between",
            alignItems: "center",
          }}
        >
          <div>
            <nav>
              <ul style={{ display: "flex", flex: 1, padding: 0 }}>
                {menuLinks.map(link => (
                  <li
                    key={link.name}
                    style={{
                      listStyleType: `none`,
                      padding: `1rem`,
                    }}
                  >
                    <Link style={{ color: `black` }} to={link.link}>
                      {link.name}
                    </Link>
                  </li>
                ))}
              </ul>
            </nav>
          </div>
        </div>
      </div>
    </header>
  );

  export default Header;