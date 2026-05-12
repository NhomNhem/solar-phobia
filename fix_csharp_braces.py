import os
import re
import sys

def get_indent(line):
    if not line.strip():
        return None
    return len(line) - len(line.lstrip())

def fix_file(path):
    with open(path, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    # Pre-process: handle namespace
    namespace_pattern = re.compile(r'^namespace\s+([\w\.]+)\s*\{?\s*$')
    processed_lines = []
    i = 0
    while i < len(lines):
        line = lines[i]
        stripped = line.strip()
        
        match = namespace_pattern.match(stripped)
        if match:
            ns_name = match.group(1)
            processed_lines.append(f"namespace {ns_name};\n")
            if "{" in stripped:
                pass
            elif i + 1 < len(lines) and lines[i+1].strip() == "{":
                i += 1
            i += 1
            continue
        processed_lines.append(line)
        i += 1

    lines = processed_lines
    final_lines = []
    stack = []
    
    keywords = ["public", "private", "protected", "internal", "class", "interface", "struct", "enum", "void", "if", "else", "for", "foreach", "while", "using", "get", "set", "static", "async"]

    for i in range(len(lines)):
        line = lines[i]
        stripped = line.strip()
        
        if not stripped:
            final_lines.append(line)
            continue
            
        curr_indent = get_indent(line)
        
        next_indent = None
        next_line_stripped = None
        for j in range(i + 1, len(lines)):
            indent = get_indent(lines[j])
            if indent is not None:
                next_indent = indent
                next_line_stripped = lines[j].strip()
                break
        
        final_lines.append(line)
        
        # 1. OPEN block
        if next_indent is not None and next_indent > curr_indent:
            first_word = stripped.split()[0] if stripped.split() else ""
            # Clean first_word from [Attribute] if it's there (though usually on separate line)
            if first_word.startswith("[") and "]" in first_word:
                # This line might start with [Attr] public class ...
                # Let's find the first real word
                parts = re.split(r'\]\s*', stripped, 1)
                if len(parts) > 1:
                    actual_stripped = parts[1]
                    first_word = actual_stripped.split()[0] if actual_stripped.split() else ""
                else:
                    first_word = ""

            is_header = False
            if first_word in keywords:
                is_header = True
            elif "(" in stripped and ")" in stripped:
                is_header = True
            
            # Additional check for property headers like "public int X"
            if not is_header and first_word and i + 1 < len(lines):
                # If next line is get/set, this is a property header
                if next_line_stripped and (next_line_stripped.startswith("get") or next_line_stripped.startswith("set")):
                    is_header = True

            if is_header:
                if stripped.endswith(";") or stripped.endswith("=>") or stripped.endswith("}"):
                    is_header = False
                elif "=" in stripped and first_word not in ["if", "for", "while", "foreach"]:
                    is_header = False
                elif stripped.startswith("//") or stripped.startswith("/*") or stripped.startswith("["):
                    is_header = False

            if is_header:
                if next_line_stripped != "{":
                    final_lines.append(" " * curr_indent + "{\n")
                    stack.append(curr_indent)
        
        # 2. CLOSE blocks
        if next_indent is not None:
            while stack and next_indent <= stack[-1]:
                level = stack.pop()
                if next_line_stripped != "}":
                    final_lines.append(" " * level + "}\n")
        else:
            while stack:
                level = stack.pop()
                final_lines.append(" " * level + "}\n")

    with open(path, 'w', encoding='utf-8') as f:
        f.writelines(final_lines)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        fix_file(sys.argv[1])
