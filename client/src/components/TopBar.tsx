type Props = {
  username: string;
  tabs: string[];
  active: string;
  onTab: (t: string) => void;
  onLogout: () => void;
};

export function TopBar({ username, tabs, active, onTab, onLogout }: Props) {
  return (
    <header className="topbar">
      <strong style={{textTransform: 'uppercase'}}>Business Esbjerg</strong>
      <nav>
        {tabs.map((t) => (
          <button key={t} onClick={() => onTab(t)} disabled={t === active}>
            {t}
          </button>
        ))}
      </nav>
      <span className="spacer">{username}</span>
      <button className="btn btn-outline" onClick={onLogout}>Log out</button>
    </header>
  );
}
